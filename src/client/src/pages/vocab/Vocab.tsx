import React, { useEffect, useState } from "react";
import { useParams } from "react-router";
import NavBar from "../../components/header/NavBar";
import { Separator } from "@radix-ui/react-separator";
import { ApiResponse, api_url } from "@/utils/api";
import { useQuery } from "@tanstack/react-query";
import { Entry, EntrySchema } from "@/types/JMDict";
import FatListCard from "../../components/vocab/FatListCard";
import { Helmet } from "react-helmet-async";

const Vocab = () => {
  let { ent_seq } = useParams();
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const { isPending, isLoading, error, data, refetch } = useQuery({
    queryKey: ["entryGet", ent_seq],
    queryFn: async () => {
      let response = await fetch(`${api_url}/Entry?ent_seq=${ent_seq}`);
      if (!response.ok) throw new Error("Get Entry by ent_seq request not OK.");
      const responseData = (await response.json()) as ApiResponse<Entry>;
      const parsed = EntrySchema.safeParse(responseData.data);
      if (!parsed.success) {
        console.error(parsed.error);
        throw new Error("Failed to parse Entry");
      }
      // console.log(parsed.data);
      return parsed.data;
    },
    enabled: false,
  });

  useEffect(() => {
    if (ent_seq) {
      refetch();
    } else {
      setErrorMsg("No search parameter provided.");
    }
  }, [ent_seq]);

  let contents = <p>Default Error</p>;

  if (errorMsg) {
    contents = <p>{errorMsg}</p>;
  } else if (isPending) {
    contents = (
      <>
        <p>PENDING... fetching {ent_seq}</p>
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
        {!data ? (
          <p>Entry ({ent_seq}) not found.</p>
        ) : (
          <>
            <h2 className="hidden">Vocab Page</h2>
            <FatListCard entry={data} />
          </>
        )}
      </>
    );
  }

  return (
    <>
      <Helmet>
        {data && (
          <title>
            {data.selectedKanjiIndex
              ? data.kanjiElements[data.selectedKanjiIndex].keb
              : data.readingElements[data.selectedReadingIndex].reb}{" "}
            - Dango Jisho
          </title>
        )}
      </Helmet>
      <div className="w-full flex flex-col items-center">
        <div className="flex flex-col gap-6 xl:w-[1000px] lg:w-[940px] md:w-[736px] w-[calc(100%-2rem)]">
          <NavBar />
          <Separator />

          <div>{contents}</div>
        </div>
      </div>
    </>
  );
};

export default Vocab;
