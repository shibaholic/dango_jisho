import { useEffect, useState } from "react";
import { useSearchParams } from "react-router";
import NavBar from "../../components/header/NavBar";
import { Separator } from "@/components/ui/separator";
import { useQuery } from "@tanstack/react-query";
import { ApiResponse, api_url, fetchSearch } from "@/utils/api";
import { z } from "zod";
import { Entry, EntrySchema } from "@/types/JMDict";
import { FatListCard } from "../../components/vocab/FatListCard";
import { Helmet } from "react-helmet-async";
import { useAuth } from "@/utils/auth";

function SearchResults() {
  let [searchParams] = useSearchParams();
  const query = searchParams.get("q") || "";
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const { tried } = useAuth();

  const searchQuery = useQuery({
    queryKey: ["wordSearch", query],
    queryFn: async () => await fetchSearch(query),
    enabled: tried,
  });

  let contents = <p>data is null or undefined</p>;

  if (errorMsg) {
    contents = <p>{errorMsg}</p>;
  } else if (searchQuery.isPending) {
    contents = (
      <>
        <p>PENDING...</p>
      </>
    );
  } else if (searchQuery.isLoading) {
    contents = (
      <>
        <p>loading...</p>
      </>
    );
  } else if (searchQuery.data) {
    contents = (
      <>
        {searchQuery.data.data.length == 0 ? (
          <p>Zero results found.</p>
        ) : (
          searchQuery.data.data.map((entry, index) => {
            return <FatListCard key={index} entry={entry} linkToVocab />;
          })
        )}
      </>
    );
  }

  return (
    <>
      <Helmet>
        <title>{query} - Dango Search </title>
      </Helmet>
      <div className="w-full flex flex-col items-center">
        <div className="flex flex-col gap-6 xl:w-[1000px] lg:w-[940px] md:w-[736px] w-[calc(100%-2rem)]">
          <NavBar />
          <Separator />
          <h2>Search Results</h2>
          <div className="flex flex-col gap-2">{contents}</div>
        </div>
      </div>
    </>
  );
}

export default SearchResults;
