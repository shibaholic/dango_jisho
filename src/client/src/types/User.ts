import { z } from "zod";

export const UserSchema = z.object({
  id: z.string(),
  username: z.string(),
  email: z.string().nullable(),
  isAdmin: z.boolean(),
});

export const UserAuthSchema = z.object({
  user: UserSchema,
  jwtToken: z.string(),
  refreshToken: z.string().nullable(),
});

export type User = z.infer<typeof UserSchema>;
export type UserAuth = z.infer<typeof UserAuthSchema>;
