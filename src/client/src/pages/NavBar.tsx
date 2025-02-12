import { Button } from "@/components/ui/button";
import { EntrySearchInput } from "@/components/ui/input";
import { forwardRef, useState, useImperativeHandle } from "react";
import { Link, useNavigate } from "react-router";

export interface NavBarProps {}

/// NavBar that appears over Landing and Search Results.
/// Contains app title, login/signup, word search bar when not logged in.
/// When logged in, additionally contains: navs for settings, my words, my stats, logout etc.
export const NavBar = (props: NavBarProps) => {
  const [query, setQuery] = useState<string>("");
  let navigate = useNavigate();

  function searchEnter() {
    navigate(`/search?q=${query}`);
  }

  return (
    <div>
      <div className="py-4 flex flex-row place-content-between items-center">
        <Link to="/">
          <h1 className="text-black">My Vocab App</h1>
        </Link>
        <Button variant={"outline"} className="text-xl py-6">
          Log in or sign up
        </Button>
      </div>
      <div className="w-full">
        <EntrySearchInput
          type="search"
          placeholder="Search a word"
          className="w-full text-xl"
          value={query}
          onChange={(e) => setQuery(e.currentTarget.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") searchEnter();
          }}
        />
      </div>
    </div>
  );
};

export default NavBar;
