export const api_url = "http://localhost:5010/api";

export type ApiResponse<T> = {
  message: string;
  status: number;
  data: T;
};
