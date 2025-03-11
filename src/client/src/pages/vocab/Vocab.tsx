import React, { useEffect, useState } from "react";
import { useParams } from "react-router";
import NavBar from "../../components/header/NavBar";
import { Separator } from "@radix-ui/react-separator";
import { fetchEntry } from "@/utils/api";
import { useQuery } from "@tanstack/react-query";
import { FatListCard } from "../../components/vocab/FatListCard";
import { Helmet } from "react-helmet-async";

const Vocab = () => {
  let { ent_seq } = useParams();
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const { isPending, isLoading, error, data, refetch } = useQuery({
    queryKey: ["entryGet", ent_seq],
    queryFn: async () => await fetchEntry(ent_seq!),
    enabled: !!ent_seq,
  });

  if (!ent_seq) {
    setErrorMsg("No vocab ID provided.");
  }

  let contents = <p>Default Error</p>;
  let helmetTitle = "Dango Jisho";

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
            <FatListCard entry={data.data} />
          </>
        )}
      </>
    );
    helmetTitle = data.data.selectedKanjiIndex
      ? data.data.kanjiElements[data.data.selectedKanjiIndex].keb
      : data.data.readingElements[data.data.selectedReadingIndex].reb +
        "- Dango Jisho";
  }

  return (
    <>
      <Helmet>{data && <title>{helmetTitle}</title>}</Helmet>
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
