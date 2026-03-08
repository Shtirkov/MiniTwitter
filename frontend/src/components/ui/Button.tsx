import type { ButtonHTMLAttributes, ReactNode } from 'react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary';
  isLoading?: boolean;
  children: ReactNode;
}

const VARIANT_CLASSES: Record<NonNullable<ButtonProps['variant']>, string> = {
  primary: [
    'bg-brand-primary text-white font-semibold',
    'hover:bg-brand-primary-hover active:scale-[0.98]',
    'disabled:opacity-50 disabled:cursor-not-allowed',
  ].join(' '),
  secondary: [
    'bg-transparent text-white font-semibold',
    'border border-gray-600 hover:border-gray-400',
    'active:scale-[0.98]',
  ].join(' '),
};

export function Button({
  variant = 'primary',
  isLoading = false,
  children,
  disabled,
  className = '',
  ...props
}: ButtonProps) {
  return (
    <button
      disabled={disabled ?? isLoading}
      className={[
        'flex w-full items-center justify-center rounded-2xl py-4 text-sm',
        'transition-all duration-150 cursor-pointer',
        VARIANT_CLASSES[variant],
        className,
      ].join(' ')}
      {...props}
    >
      {isLoading ? <LoadingSpinner /> : children}
    </button>
  );
}

function LoadingSpinner() {
  return (
    <span className="flex items-center gap-2">
      <svg
        className="h-4 w-4 animate-spin"
        viewBox="0 0 24 24"
        fill="none"
        aria-hidden="true"
      >
        <circle
          className="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          strokeWidth="4"
        />
        <path
          className="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
        />
      </svg>
      Loading…
    </span>
  );
}
