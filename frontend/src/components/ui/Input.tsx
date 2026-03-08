import { forwardRef } from 'react';
import type { InputHTMLAttributes, ReactNode } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  leftIcon?: ReactNode;
  rightSlot?: ReactNode;
  error?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, leftIcon, rightSlot, error, id, ...props }, ref) => {
    const inputId = id ?? label.toLowerCase().replace(/\s+/g, '-');

    return (
      <div className="flex flex-col gap-2">
        <label
          htmlFor={inputId}
          className="text-[11px] font-semibold uppercase tracking-[0.15em] text-gray-400"
        >
          {label}
        </label>

        <div
          className={[
            'flex items-center gap-3 rounded-2xl px-4 py-[14px]',
            'bg-brand-surface border',
            error ? 'border-red-500' : 'border-brand-border',
            'transition-colors focus-within:border-brand-primary',
          ].join(' ')}
        >
          {leftIcon && <span className="shrink-0">{leftIcon}</span>}

          <input
            ref={ref}
            id={inputId}
            className="min-w-0 flex-1 bg-transparent text-sm text-white placeholder-gray-500 outline-none"
            {...props}
          />

          {rightSlot && <span className="shrink-0">{rightSlot}</span>}
        </div>

        {error && <p className="text-xs text-red-400">{error}</p>}
      </div>
    );
  },
);

Input.displayName = 'Input';
