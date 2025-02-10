import { ApiResponse, api_url } from "@/Api";
import { useQuery } from "@tanstack/react-query";
import { forwardRef, useEffect, useImperativeHandle, useState } from "react";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";

import { ChevronsUpDown, ArrowDownToLine } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { TrackedEntry, TrackedEntrySchema } from "@/types/TrackedEntry";
import ObjectDisplay from "./objectDisplay";
import { Entry, EntrySchema } from "@/types/JMDict";
// import CardBackSmall from "../card-back/card-back";

export interface DetailsRef {
  // fetchData: (es: string) => void;
  setES: (es: string) => void;
}

export const Details = forwardRef((props, ref: React.Ref<DetailsRef>) => {
  const [ent_seq, setEnt_seq] = useState<string | null>(null);

  useImperativeHandle(ref, () => ({
    setES: (es: string) => {
      setEnt_seq(es);
    },
  }));

  return (
    <>
      <div key={ent_seq}>
        {ent_seq ? (
          <>
            <Section
              title="TrackedEntry details"
              queryKeyName="trackedEntry"
              ent_seq={ent_seq}
              queryFn={async (): Promise<TrackedEntry> => {
                const response = await fetch(
                  `${api_url}/TrackedEntry?ent_seq=${ent_seq}`
                );
                if (!response.ok) throw new Error("Failed to fetch data");
                const data =
                  (await response.json()) as ApiResponse<TrackedEntry>;
                const parsed = TrackedEntrySchema.safeParse(data.data);
                if (!parsed.success) {
                  console.error(parsed.error.format);
                  throw new Error("Failed to parse TrackedEntry");
                }
                return parsed.data;
              }}
            />
            <Section
              title="Entry details"
              queryKeyName="entry"
              ent_seq={ent_seq}
              queryFn={async (): Promise<Entry> => {
                const response = await fetch(
                  `${api_url}/Entry?ent_seq=${ent_seq}`
                );
                if (!response.ok) throw new Error("Failed to fetch data");
                const data = (await response.json()) as ApiResponse<Entry>;
                const parsed = EntrySchema.safeParse(data.data);
                if (!parsed.success) {
                  console.error(parsed.error.format);
                  throw new Error("Failed to parse Entry");
                }
                return parsed.data;
              }}
            />
          </>
        ) : (
          <p>Click "Show details"</p>
        )}
      </div>
    </>
  );
});

interface SectionProps<T> {
  title: string;
  ent_seq: string;
  queryKeyName: string;
  queryFn: () => Promise<T>;
}

const Section = <T,>({
  title,
  ent_seq,
  queryKeyName,
  queryFn,
}: SectionProps<T>) => {
  const [data, setData] = useState<T | null>(null);
  const [status, setStatus] = useState<string>("undefined");
  const [open, setOpen] = useState<boolean>(true);

  const { refetch, isError, error } = useQuery({
    queryKey: [queryKeyName, ent_seq],
    queryFn: queryFn,
    enabled: false,
  });

  async function loadSection() {
    const freshData = await refetch();

    if (isError) {
      setStatus("load data error");
      console.error(error.message);
    } else {
      if (freshData.data) {
        setData(freshData.data);
        setStatus("loaded");
        // setOpen(true);
      }
    }
  }

  useEffect(() => {
    loadSection();
  }, []);

  return (
    <>
      <Collapsible className="w-full" open={open}>
        <div className="flex w-full mb-4 gap-4 items-center">
          <h4 className="max-w-1/2 text-md font-bold">{title}</h4>
          {status === "loaded" ? (
            <CollapsibleTrigger asChild className="flex-grow">
              <Button
                onClick={(e) => {
                  e.preventDefault();
                  setOpen(!open);
                }}
                variant="outline"
                size="sm"
              >
                <ChevronsUpDown className="h-full w-full" />
                <span className="sr-only">Toggle</span>
              </Button>
            </CollapsibleTrigger>
          ) : (
            <Button
              onClick={loadSection}
              variant="outline"
              className="flex-grow"
              size="sm"
            >
              <ArrowDownToLine className="h-full w-full" />
              <span className="sr-only">Fetch</span>
            </Button>
          )}
        </div>

        <CollapsibleContent className="font-mono">
          {data && <ObjectDisplay label={queryKeyName} data={data} />}
        </CollapsibleContent>

        <Separator className="my-4" />
      </Collapsible>
    </>
  );
};

// interface HeaderValueProps<T> {
//   header: string;
//   // field: string | null | undefined;
//   field: keyof T;
//   modifier?: (arg0: any) => string;
// }

// const HeaderValue = <T,>({ header, field, modifier }: HeaderValueProps<T>) => {
//   const { data } = useSectionContext<T>();

//   return (
//     <>
//       <p className="mb-4 font-medium">
//         {header}:{" "}
//         <span className="font-normal font-mono">
//           {data[field]
//             ? modifier
//               ? modifier(data[field])
//               : data[field] + ""
//             : "null"}
//         </span>
//       </p>
//     </>
//   );
// };
