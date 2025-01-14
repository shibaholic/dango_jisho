export const api_url = "http://localhost:5010";

export type ApiResponse<T> = {
  message: string;
  status: number;
  data: T;
};
