import { Button } from "@/components/ui/button";
import { EntrySearchInput } from "@/components/ui/input";
import { forwardRef, useState, useImperativeHandle, useRef } from "react";
import { Link, useNavigate } from "react-router";
import LogInModal, { AuthModalHandle } from "../auth/logInModal";
import { useAuth } from "@/utils/auth";
import { User } from "lucide-react";
import { UserBadge } from "./UserBadge";

export interface NavBarProps {
  noSearchBar?: boolean | undefined;
}

/// NavBar that appears over Landing and Search Results.
/// Contains app title, login/signup, word search bar when not logged in.
/// When logged in, additionally contains: navs for settings, my words, my stats, logout etc.
export const NavBar = (props: NavBarProps) => {
  const [query, setQuery] = useState<string>("");
  const { user } = useAuth();

  let navigate = useNavigate();

  const authModalRef = useRef<AuthModalHandle>(null);

  function searchEnter() {
    navigate(`/search?q=${query}`);
  }

  return (
    <div>
      <div className="py-4 flex flex-row place-content-between items-center">
        <Link className="hover:bg-gray-100 px-3 py-1 pb-2 rounded-lg" to="/">
          <h1 className="text-black text-[40px]">My Vocab App</h1>
        </Link>
        {user ? (
          <UserBadge user={user} />
        ) : (
          <Button
            variant={"outline"}
            className="text-xl py-6"
            onClick={(e) => authModalRef.current?.open()}
          >
            Log in or sign up
          </Button>
        )}
      </div>
      {(props.noSearchBar === undefined || props.noSearchBar === false) && (
        <div className="w-full">
          <EntrySearchInput
            type="search"
            placeholder="Search a word"
            className="w-full text-xl focus:bg-inherit hover:bg-gray-100"
            value={query}
            onChange={(e) => setQuery(e.currentTarget.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") searchEnter();
            }}
          />
        </div>
      )}
      <LogInModal ref={authModalRef} />
    </div>
  );
};

export default NavBar;
