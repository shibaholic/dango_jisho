import NavBar from "@/components/header/NavBar";
import { Tag } from "@/types/Tag";
import api, { ApiResponse, api_url, fetchTag_EITs } from "@/utils/api";
import { useAuth } from "@/utils/auth";
import { Separator } from "@radix-ui/react-separator";
import { keepPreviousData, useQuery } from "@tanstack/react-query";
import React, { useEffect, useRef, useState } from "react";
import { Helmet } from "react-helmet-async";
import { z } from "zod";
import TagShowcase from "./TagShowcase";
import { Button } from "@/components/ui/button";
import NewTagModal, { NewTagModalHandle } from "@/components/tags/NewTagModal";

const Tags = () => {
  const { user } = useAuth();

  const newTagRef = useRef<NewTagModalHandle>(null);

  // TODO: optimization: replace tag-eits with a new Dto that
  // only contains tagId and it's eits.
  const query = useQuery({
    queryKey: ["tag-eits", user?.id],
    queryFn: async () => await fetchTag_EITs(),
    enabled: !!user,
  });

  let contents = <p>data is null or undefined</p>;

  if (query.isLoading) contents = <p>Loading...</p>;
  else if (query.error) contents = <p>Error: {query.error.message}</p>;
  else if (query.isSuccess) {
    contents = (
      <>
        {query.data.data.map((tag, index) => {
          return <TagShowcase key={index} tag={tag} />;
        })}
      </>
    );
  }

  return (
    <>
      <Helmet>
        <title> Your Tags - Dango Jisho </title>
      </Helmet>
      <div className="w-full flex flex-col items-center">
        <div className="flex flex-col gap-6 xl:w-[1000px] lg:w-[940px] md:w-[736px] w-[calc(100%-2rem)] pb-4">
          <NavBar />
          <Separator />
          <h2>Your Tags</h2>
          <p>
            Tags are for organizing words by different categories that can be
            non-mutally exclusive. These tags are later added to StudySets where
            you actually review the words.
          </p>
          <div>
            <Button
              className="w-full"
              variant="outline"
              onClick={() => newTagRef.current?.open()}
            >
              New tag
            </Button>
          </div>
          <div className="flex flex-col w-full gap-2">{contents}</div>
        </div>
      </div>
      <NewTagModal ref={newTagRef} refreshTags={query.refetch} />
    </>
  );
};

export default Tags;
