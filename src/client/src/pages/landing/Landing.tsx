import { Button } from "@/components/ui/button";
import { EntrySearchInput, Input } from "@/components/ui/input";

function Landing() {
  return (
    <div className="w-full flex flex-col items-center">
      <div className="w-[1000px]">
        <div className="px-4 py-4 flex flex-row place-content-between items-center">
          <h1 className="">My Vocab App</h1>
          <Button>Log in or sign up</Button>
        </div>
        <div className="w-full">
          <EntrySearchInput
            type="search"
            placeholder="Search a word"
            className="w-full text-xl"
          />
        </div>
        <div>
          <h2>Study sets and tags</h2>
        </div>
      </div>
    </div>
  );
}

export default Landing;
