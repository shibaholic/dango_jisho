import { Button } from "@/components/ui/button";
import { EntrySearchInput } from "@/components/ui/input";
import { forwardRef, useState, useImperativeHandle, useRef } from "react";
import { Link, useNavigate } from "react-router";
import LoginModal, { AuthModalHandle } from "../auth/LoginModal";
import { useAuth } from "@/utils/auth";
import { User } from "lucide-react";
import { UserBadge } from "./UserBadge";
import SignupModal from "../auth/SignupModal";

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

  const loginModalRef = useRef<AuthModalHandle>(null);
  const signupModalRef = useRef<AuthModalHandle>(null);

  function searchEnter() {
    navigate(`/search?q=${query}`);
  }

  let contents = <p>default navbar</p>;
  if (!user) {
    contents = (
      <div className="flex flex-row gap-2">
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={(e) => loginModalRef.current?.open()}
        >
          Log in
        </Button>
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={(e) => signupModalRef.current?.open()}
        >
          Signup
        </Button>
      </div>
    );
  } else {
    contents = (
      <div className="flex flex-row gap-2">
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={() => navigate("/tags")}
        >
          Tags
        </Button>
        <UserBadge user={user} />
      </div>
    );
  }

  return (
    <div>
      <div className="py-4 flex flex-row place-content-between items-center">
        <Link className="hover:bg-gray-100 px-3 py-1 pb-2 rounded-lg" to="/">
          <h1 className="text-black text-[40px]">🍡 Dango Jisho</h1>
        </Link>
        {contents}
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
      <LoginModal ref={loginModalRef} />
      <SignupModal ref={signupModalRef} />
    </div>
  );
};

export default NavBar;
