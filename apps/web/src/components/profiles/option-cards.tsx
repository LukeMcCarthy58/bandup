"use client";

import { useId } from "react";

import { FieldMessage } from "@/components/ui/field";
import { cn } from "@/lib/utils";

type Option<T extends string> = { value: T; label: string; hint: string };

/** A single choice shown as large, thumb-friendly cards (native radio buttons underneath). */
export function OptionCards<T extends string>({
  name,
  legend,
  options,
  value,
  onChange,
  error,
}: {
  name: string;
  legend: string;
  options: Option<T>[];
  value: T | null;
  onChange: (value: T) => void;
  error?: string;
}) {
  const messageId = useId();

  return (
    <fieldset aria-describedby={error ? messageId : undefined}>
      <legend className="mb-1.5 text-sm font-semibold">{legend}</legend>
      <div className="grid gap-2 sm:grid-cols-2">
        {options.map((option) => (
          <label
            key={option.value}
            className={cn(
              "flex min-h-11 cursor-pointer items-start gap-3 rounded-lg border p-3 transition-colors",
              value === option.value
                ? "border-primary bg-secondary"
                : "border-input bg-card hover:bg-muted",
            )}
          >
            <input
              type="radio"
              name={name}
              value={option.value}
              checked={value === option.value}
              onChange={() => onChange(option.value)}
              className="mt-1 size-4 accent-[var(--primary)]"
            />
            <span>
              <span className="block font-semibold">{option.label}</span>
              <span className="block text-sm text-muted-foreground">{option.hint}</span>
            </span>
          </label>
        ))}
      </div>
      <FieldMessage id={messageId} error={error} />
    </fieldset>
  );
}
