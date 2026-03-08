import { Logo } from '../components/Logo';
import { LoginForm } from '../components/auth/LoginForm';

export function LoginPage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-brand-bg px-6 py-10">
      <div className="flex w-full max-w-sm flex-col gap-8">
        <Logo />

        <div className="flex flex-col gap-1">
          <h1 className="text-4xl font-bold text-white">Welcome back</h1>
          <p className="text-[15px] text-gray-400">Sign in to continue to your feed</p>
        </div>

        <LoginForm />
      </div>
    </main>
  );
}
