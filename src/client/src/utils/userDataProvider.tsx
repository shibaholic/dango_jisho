import { Tag } from "@/types/Tag";
import {
  ReactNode,
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";
import { useAuth } from "./auth";
import { fetchTag_EITs, fetchTags } from "./api";
import { keepPreviousData, useQuery } from "@tanstack/react-query";

interface UserDataContextType {
  tags: Tag[] | null;
}

const UserDataContext = createContext<UserDataContextType | undefined>(
  undefined
);

interface UserDataProps {
  children: ReactNode;
}

export const UserDataProvider: React.FC<UserDataProps> = ({ children }) => {
  const [tags, setTags] = useState<Tag[] | null>(null);

  const { user } = useAuth();

  const value: UserDataContextType = {
    tags,
  };

  const query = useQuery({
    queryKey: ["tags", user?.id],
    queryFn: async () => await fetchTags(),
    enabled: !!user,
    placeholderData: keepPreviousData,
  });

  useEffect(() => {
    if (query.error) {
    } else {
      if (query.data?.data) setTags(query.data?.data);
    }
  }, [query.data]);

  return (
    <UserDataContext.Provider value={value}>
      {children}
    </UserDataContext.Provider>
  );
};

// Custom hook for consuming the AuthContext
export const useUserData = (): UserDataContextType => {
  const context = useContext(UserDataContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
