export type User = {
    id: number;
    username: string;
    email: string;
    roles?: string[];
};

export type LoginRequest = { usernameOrEmail: string; password: string };
export type RegisterRequest = { username: string; email: string; password: string };

export type AuthResponse = {
    user?: User;
};
