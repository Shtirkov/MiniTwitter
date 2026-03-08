export function Logo() {
  return (
    <div className="flex items-center gap-3">
      <div
        className="flex h-12 w-12 items-center justify-center rounded-xl text-xl font-bold text-white"
        style={{ background: 'linear-gradient(135deg, #9333ea, #6d28d9)' }}
        aria-hidden="true"
      >
        M
      </div>
      <span className="text-2xl font-bold tracking-tight">
        <span className="text-white">Mini</span>
        <span className="text-brand-cyan">Twitter</span>
      </span>
    </div>
  );
}
