"use client";

import type { Schemas } from "@bandup/api-client";
import { MapPin } from "lucide-react";
import { useEffect, useId, useState } from "react";

import { FieldMessage, Input, Label } from "@/components/ui/field";
import { cn } from "@/lib/utils";

export type Town = Schemas["TownSummary"];

export const townLabel = (town: Town) => [town.name, town.region].filter(Boolean).join(", ");

type TownPickerProps = {
  apiUrl: string;
  value: Town | null;
  onChange: (town: Town | null) => void;
  error?: string;
};

/**
 * Accessible autocomplete (WAI-ARIA combobox) over GET /api/v1/towns. Only a town is
 * ever chosen: never a street or postcode (privacy rule).
 */
export function TownPicker({ apiUrl, value, onChange, error }: TownPickerProps) {
  const inputId = useId();
  const listId = useId();
  const messageId = useId();
  const [query, setQuery] = useState(value ? townLabel(value) : "");
  const [results, setResults] = useState<Town[]>([]);
  const [open, setOpen] = useState(false);
  const [active, setActive] = useState(-1);
  const [status, setStatus] = useState<"idle" | "loading" | "failed">("idle");
  // The label of the chosen town; while the input shows it, there's nothing to search.
  const [selectedLabel, setSelectedLabel] = useState(value ? townLabel(value) : "");

  // Only search when there's something new to look up (2+ characters, not the chosen town).
  const term = query.trim();
  const searchable = term.length >= 2 && term !== selectedLabel;
  const shown = searchable ? results : [];
  const showList = open && shown.length > 0;
  const noMatches = open && searchable && status === "idle" && shown.length === 0;

  useEffect(() => {
    if (!searchable) return;

    const controller = new AbortController();
    const timer = setTimeout(async () => {
      setStatus("loading");
      try {
        const url = `${apiUrl}/api/v1/towns?query=${encodeURIComponent(term)}&limit=8`;
        const response = await fetch(url, { signal: controller.signal });
        if (!response.ok) throw new Error(String(response.status));
        setResults((await response.json()) as Town[]);
        setActive(-1);
        setOpen(true);
        setStatus("idle");
      } catch (e) {
        if (!controller.signal.aborted) setStatus(e instanceof Error ? "failed" : "idle");
      }
    }, 250);

    return () => {
      clearTimeout(timer);
      controller.abort();
    };
  }, [apiUrl, term, searchable]);

  function choose(town: Town) {
    const label = townLabel(town);
    setSelectedLabel(label);
    setQuery(label);
    setResults([]);
    setOpen(false);
    onChange(town);
  }

  function onKeyDown(event: React.KeyboardEvent<HTMLInputElement>) {
    if (!showList) return;
    if (event.key === "ArrowDown") {
      event.preventDefault();
      setActive((i) => (i + 1) % shown.length);
    } else if (event.key === "ArrowUp") {
      event.preventDefault();
      setActive((i) => (i <= 0 ? shown.length - 1 : i - 1));
    } else if (event.key === "Enter" && active >= 0) {
      event.preventDefault();
      choose(shown[active]);
    } else if (event.key === "Escape") {
      setOpen(false);
    }
  }

  return (
    <div className="relative">
      <Label htmlFor={inputId}>Town or area</Label>
      <div className="relative">
        <MapPin
          aria-hidden
          className="pointer-events-none absolute top-1/2 left-3 size-5 -translate-y-1/2 text-muted-foreground"
        />
        <Input
          id={inputId}
          role="combobox"
          autoComplete="off"
          aria-autocomplete="list"
          aria-expanded={showList}
          aria-controls={listId}
          aria-activedescendant={active >= 0 ? `${listId}-${active}` : undefined}
          aria-invalid={!!error}
          aria-describedby={messageId}
          placeholder="Start typing, e.g. Manchester"
          className="pl-10"
          value={query}
          onChange={(event) => {
            setQuery(event.target.value);
            if (value) {
              setSelectedLabel("");
              onChange(null);
            }
          }}
          onKeyDown={onKeyDown}
          onBlur={() => setTimeout(() => setOpen(false), 100)}
          onFocus={() => shown.length > 0 && setOpen(true)}
        />
      </div>
      <input type="hidden" name="townId" value={value?.id ?? ""} />

      {showList ? (
        <ul
          id={listId}
          role="listbox"
          className="absolute z-30 mt-1 max-h-72 w-full overflow-auto rounded-md border bg-popover py-1 shadow-lg"
        >
          {shown.map((town, index) => (
            <li
              key={town.id}
              id={`${listId}-${index}`}
              role="option"
              aria-selected={index === active}
              onMouseDown={(event) => {
                event.preventDefault();
                choose(town);
              }}
              className={cn(
                "flex min-h-11 cursor-pointer flex-col justify-center px-3 py-1.5",
                index === active ? "bg-secondary text-secondary-foreground" : "hover:bg-muted",
              )}
            >
              <span className="font-medium">{town.name}</span>
              <span className="text-sm text-muted-foreground">
                {[town.region, town.country].filter(Boolean).join(", ")}
              </span>
            </li>
          ))}
        </ul>
      ) : null}

      <FieldMessage
        id={messageId}
        error={
          error ??
          (status === "failed" ? "Town search isn't working right now. Try again." : undefined)
        }
      >
        {noMatches
          ? "No towns match that yet. Try a nearby larger town."
          : "We only show your town, never your address."}
      </FieldMessage>
    </div>
  );
}
