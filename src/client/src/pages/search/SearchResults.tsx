import { useEffect, useState } from "react";
import { useSearchParams } from "react-router";
import NavBar from "../../components/header/NavBar";
import { Separator } from "@/components/ui/separator";
import { useQuery } from "@tanstack/react-query";
import { ApiResponse, api_url } from "@/utils/api";
import { z } from "zod";
import { Entry, EntrySchema } from "@/types/JMDict";
import FatListCard from "../../components/vocab/FatListCard";
import { Helmet } from "react-helmet-async";

function SearchResults() {
  let [searchParams] = useSearchParams();
  const query = searchParams.get("q") || "";
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const { isPending, isLoading, error, data, refetch } = useQuery({
    queryKey: ["wordSearch", query],
    queryFn: async () => {
      let response = await fetch(`${api_url}/Entry/search?query=${query}`);
      if (!response.ok) throw new Error("Word search query not OK.");
      const responseData = (await response.json()) as ApiResponse<Entry[]>;
      const entryArraySchema = z.array(EntrySchema);
      const parsed = entryArraySchema.safeParse(responseData.data);
      if (!parsed.success) {
        console.error(parsed.error);
        throw new Error("Failed to parse Entry[]");
      }
      // console.log(parsed.data);
      return parsed.data;
    },
    enabled: false,
  });

  useEffect(() => {
    if (query) {
      refetch();
    } else {
      setErrorMsg("No search parameter provided.");
    }
  }, [query]);

  let contents = <p>data is null or undefined</p>;

  if (errorMsg) {
    contents = <p>{errorMsg}</p>;
  } else if (isPending) {
    contents = (
      <>
        <p>PENDING...</p>
      </>
    );
  } else if (isLoading) {
    contents = (
      <>
        <p>loading...</p>
      </>
    );
  } else if (data) {
    contents = (
      <>
        {data.length == 0 ? (
          <p>Zero results found.</p>
        ) : (
          data.map((entry, index) => {
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
          <div className="flex flex-col gap-1">{contents}</div>
        </div>
      </div>
    </>
  );
}

export default SearchResults;
