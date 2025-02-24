import { User } from "@/types/User";
import axios, { AxiosError } from "axios";

export const api_url = "http://localhost:5010/api";

export type ApiResponse<T> = {
  message: string;
  status: number;
  data: T;
};

// making a api object that automatically tries to refresh token, if access token expired.

const api = axios.create({
  baseURL: `${api_url}`,
  withCredentials: true,
});

api.interceptors.response.use(
  (response) => response,
  async (error: any) => {
    if (error.response?.status === 401) {
      // attempt to refresh the token, because original request received 401
      try {
        await axios.post(`${api_url}/auth/refresh`, null, {
          withCredentials: true,
        });
        console.log("After-401-auto-fetch success.");
        return api.request(error.config); // Retry the original request
      } catch (refreshError) {
        let errorMsg = "After-401-auto-refresh error: ";
        if (axios.isAxiosError(refreshError)) {
          // Server responded with status code not in 2xx range
          if (refreshError.response) {
            switch (refreshError.response.status) {
              case 400:
                // if 400 then there was no refresh token, follow client-side authorization rules, since it could be a new user.
                errorMsg += "Possible new user (Bad request).";
                break;
              case 401:
                // if 401 then refresh token expired, so just open the loginModal?
                errorMsg += "Invalid or expired refresh token (Unauthorized).";
                break;
              default:
                errorMsg += "Not tracked non-2xx error.";
                break;
            }
          } else if (refreshError.request) {
            errorMsg += "Network error.";
          }
        } else {
          errorMsg = "Unknown error.";
        }

        return Promise.reject(errorMsg); // 🔴 Fail if refresh fails
      }
    }
    // other non-2xx errors
    return Promise.reject(error);
  }
);

export async function fetchRefreshTokenNewSession(): Promise<User> {
  const response = await axios
    .post<User>(`${api_url}/auth/refresh`, null, {
      withCredentials: true,
    })
    .then((data) => {
      return Promise.resolve(data);
    })
    .catch((error) => {
      let errorMsg = "New session refresh token error: ";
      if (axios.isAxiosError(error)) {
        // Server responded with status code not in 2xx range
        if (error.response) {
          switch (error.response.status) {
            case 400:
              errorMsg += "Possible new user (Bad request).";
              break;
            case 401:
              errorMsg += error.response.data as string;
              // should delete accessToken and refreshToken here, by trying to logout.
              break;
            default:
              errorMsg += "Not tracked non-2xx error.";
          }
        } else if (error.request) {
          errorMsg = "Network error.";
        }
      } else {
        errorMsg = "Unknown error.";
      }
      console.error(errorMsg);
      return Promise.reject(); // somehow remains uncaught by something but not printed to console...
    });

  return response.data;
}

export async function fetchLogout() {
  const response = await api.post(`${api_url}/auth/logout`);

  return;
}

export async function fetchUserAuth() {
  const response = await api.get(`/auth/user`, {
    withCredentials: true,
  });

  return response.data;
}

export default api;
