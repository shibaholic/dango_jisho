import NavBar from "@/components/header/NavBar";
import { Tag, TagSchema } from "@/types/Tag";
import api, { ApiResponse, api_url, fetchTags } from "@/utils/api";
import { useAuth } from "@/utils/auth";
import { Separator } from "@radix-ui/react-separator";
import { useQuery } from "@tanstack/react-query";
import React, { useEffect, useState } from "react";
import { Helmet } from "react-helmet-async";
import { z } from "zod";
import TagShowcase from "./TagShowcase";

const Tags = () => {
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const { user } = useAuth();

  const { isLoading, error, data } = useQuery({
    queryKey: ["tags", user?.id],
    queryFn: async () => {
      let response = await fetchTags();
      if (!response) throw new Error("Tags query not OK.");
      const tagArraySchema = z.array(TagSchema);
      const parsed = tagArraySchema.safeParse(response.data);
      if (!parsed.success) {
        console.error(parsed.error);
        throw new Error("Failed to parse Tag[]");
      }
      // console.log(parsed.data);
      return parsed.data;
    },
    enabled: !!user,
  });

  let contents = <p>data is null or undefined</p>;

  if (isLoading) contents = <p>Loading...</p>;
  else if (error) contents = <p>Error: {error.message}</p>;
  else if (data) {
    contents = (
      <>
        {data?.map((tag, index) => {
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
        <div className="flex flex-col gap-6 xl:w-[1000px] lg:w-[940px] md:w-[736px] w-[calc(100%-2rem)]">
          <NavBar />
          <Separator />
          <h2>Your Tags</h2>
          <p>
            Tags are for organizing words by different categories that can be
            non-mutally exclusive. These tags are later added to StudySets where
            you actually review the words.
          </p>
          <div className="flex flex-col w-full gap-2">{contents}</div>
        </div>
      </div>
    </>
  );
};

export default Tags;
