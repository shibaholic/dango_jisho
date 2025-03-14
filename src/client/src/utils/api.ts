import { Entry } from "@/types/JMDict";
import { Tag } from "@/types/Tag";
import { TrackedEntry, TrackedEntrySchema } from "@/types/TrackedEntry";
import { User } from "@/types/User";
import axios, { AxiosError } from "axios";
import { z } from "zod";

export const api_url = import.meta.env.VITE_API_URL;

export type ApiResponse<T> = {
  message: string;
  status: number;
  data: T;
  pageIndex: number | null;
  pageSize: number | null;
  resultCount: number | null;
  totalElements: number | null;
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
  const response = await api.get(`/auth/user`);

  return response.data;
}

export async function fetchEntry(ent_seq: string): Promise<ApiResponse<Entry>> {
  const response = await api.get(`/entry/${ent_seq}`);

  return response.data;
}

export async function fetchTag_EITs(): Promise<ApiResponse<TrackedEntry[]>> {
  const response = await api.get(`/tags/entryIsTagged`);

  return response.data;
}

export async function fetchTags(): Promise<ApiResponse<Tag[]>> {
  const response = await api.get(`/tags`);

  return response.data;
}

// use this one for paginated tag entry queries
export async function fetchTagTrackedEntries(
  tagId: string,
  pageIndex: number,
  pageSize: number
): Promise<ApiResponse<TrackedEntry[]>> {
  const response = await api.get(
    `/trackedentry/tag/${tagId}?pageIndex=${pageIndex}&pageSize=${pageSize}`
  );

  return response.data;
}

export async function fetchSearch(
  query: string
): Promise<ApiResponse<Entry[]>> {
  const response = await api.get(`/entry/search?query=${query}`);

  return response.data;
}

export async function fetchNextReview(
  tagId: string
): Promise<ApiResponse<TrackedEntry>> {
  const response = await api.get(`/review/tag/${tagId}`);

  return response.data;
}

export async function fetchTrackedEntry(
  ent_seq: string
): Promise<ApiResponse<TrackedEntry>> {
  const response = await api.get(`/trackedentry/${ent_seq}`);

  return response.data;
}

export interface CommandEntryTagsProps {
  ent_seq: string;
  tagValues: Record<string, boolean>;
}

export async function commandEntryTags({
  ent_seq,
  tagValues,
}: CommandEntryTagsProps) {
  await api.post(`/entry/${ent_seq}/tags`, { tagValues: tagValues });

  return;
}

export interface NewTagInput {
  name: string;
}

export async function commandNewTag(data: NewTagInput) {
  await api.post(`/tag`, data);

  return;
}

export interface EntryEventInput {
  ent_seq: string;
  value: string;
}

export async function commandReviewEvent(data: EntryEventInput) {
  await api.post(`/review`, { ...data, eventType: "Review" });

  return;
}

export async function commandChangeEvent(data: EntryEventInput) {
  await api.post(`/review`, { ...data, eventType: "Change" });

  return;
}

export default api;
