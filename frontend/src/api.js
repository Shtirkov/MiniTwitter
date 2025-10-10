export const API_BASE =
    (import.meta.env.VITE_API_BASE_URL && import.meta.env.VITE_API_BASE_URL.replace(/\/+$/, ''))
    || 'http://localhost:8080/api';
