import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { User, Lock, GripVertical } from 'lucide-react';
import { Input } from '../ui/Input';
import { Button } from '../ui/Button';
import { Divider } from '../ui/Divider';
import { useLogin } from '../../hooks/useLogin';
import { ROUTES } from '../../utils/constants';

const loginSchema = z.object({
  email: z.string().email('Please enter a valid email address'),
  password: z.string().min(1, 'Password is required'),
});

type LoginFormValues = z.infer<typeof loginSchema>;

function InputBadge() {
  return (
    <div
      className="flex h-8 w-8 items-center justify-center rounded-lg bg-brand-badge"
      aria-hidden="true"
    >
      <GripVertical size={14} className="text-white" />
    </div>
  );
}

export function LoginForm() {
  const { error, isLoading, submit } = useLogin();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
  });

  return (
    <form
      onSubmit={handleSubmit(submit)}
      noValidate
      className="flex flex-col gap-5"
      aria-label="Login form"
    >
      <Input
        label="Email"
        type="email"
        placeholder="you@example.com"
        autoComplete="email"
        leftIcon={<User size={18} className="text-purple-400" />}
        rightSlot={<InputBadge />}
        error={errors.email?.message}
        {...register('email')}
      />

      <Input
        label="Password"
        type="password"
        placeholder="••••••••"
        autoComplete="current-password"
        leftIcon={<Lock size={18} className="text-amber-400" />}
        rightSlot={<InputBadge />}
        error={errors.password?.message}
        {...register('password')}
      />

      {error && (
        <p role="alert" className="text-center text-sm text-red-400">
          {error}
        </p>
      )}

      <Button type="submit" isLoading={isLoading} className="mt-1">
        Sign In
      </Button>

      <Divider label="or" />

      <Button variant="secondary" type="button">
        Continue with Google
      </Button>

      <p className="text-center text-sm text-gray-400">
        Don&apos;t have an account?{' '}
        <Link
          to={ROUTES.REGISTER}
          className="font-medium text-violet-400 hover:text-violet-300 transition-colors"
        >
          Create one
        </Link>
      </p>
    </form>
  );
}
