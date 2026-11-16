"use client";

import { Check } from "lucide-react";
import { useId, useState } from "react";

import { FieldMessage } from "@/components/ui/field";
import { cn } from "@/lib/utils";

type Option = { id: number; name: string };

type ChipSelectProps = {
  name: string;
  legend: string;
  hint?: string;
  options: Option[];
  /** Selected ids in the order they were picked. */
  selected: number[];
  onChange: (ids: number[]) => void;
  max: number;
  /** Badge for the first selection, e.g. "Main" for a main instrument. */
  firstLabel?: string;
  error?: string;
};

/** Multi-select as tappable chips. Selections are submitted as hidden inputs, in order. */
export function ChipSelect({
  name,
  legend,
  hint,
  options,
  selected,
  onChange,
  max,
  firstLabel,
  error,
}: ChipSelectProps) {
  const messageId = useId();
  const [limitHit, setLimitHit] = useState(false);

  function toggle(id: number) {
    if (selected.includes(id)) {
      setLimitHit(false);
      onChange(selected.filter((s) => s !== id));
    } else if (selected.length >= max) {
      setLimitHit(true);
    } else {
      setLimitHit(false);
      onChange([...selected, id]);
    }
  }

  const message = error ?? (limitHit ? `You can choose up to ${max}.` : undefined);

  return (
    <fieldset aria-describedby={messageId}>
      <legend className="mb-1.5 text-sm font-semibold">{legend}</legend>
      <ul className="flex flex-wrap gap-2">
        {options.map((option) => {
          const position = selected.indexOf(option.id);
          const isSelected = position !== -1;
          return (
            <li key={option.id}>
              <button
                type="button"
                aria-pressed={isSelected}
                onClick={() => toggle(option.id)}
                className={cn(
                  "inline-flex min-h-11 items-center gap-1.5 rounded-full border px-4 text-sm font-medium transition-colors",
                  isSelected
                    ? "border-primary bg-secondary text-secondary-foreground"
                    : "border-input bg-card text-foreground hover:bg-muted",
                )}
              >
                {isSelected ? <Check aria-hidden className="size-4" /> : null}
                {option.name}
                {isSelected && position === 0 && firstLabel ? (
                  <span className="rounded-full bg-primary px-1.5 text-[11px] text-primary-foreground">
                    {firstLabel}
                  </span>
                ) : null}
              </button>
            </li>
          );
        })}
      </ul>
      {selected.map((id) => (
        <input key={id} type="hidden" name={name} value={id} />
      ))}
      <FieldMessage id={messageId} error={message}>
        {hint}
      </FieldMessage>
    </fieldset>
  );
}
