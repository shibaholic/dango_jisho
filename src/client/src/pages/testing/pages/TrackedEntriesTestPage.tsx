import {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";
import { DataTable, TableRef } from "../components/DataTable";
import { useQuery } from "@tanstack/react-query";
import { ApiResponse, api_url } from "@/utils/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { DetailsSection } from "../components/DetailsSection";
import { TrackedEntry, TrackedEntrySchema } from "@/types/TrackedEntry";
import { ColumnDef } from "@tanstack/react-table";
import { Entry, EntrySchema } from "@/types/JMDict";

const columns: ColumnDef<TrackedEntry>[] = [
  {
    header: "ent_seq",
    accessorKey: "ent_seq",
  },
  {
    header: "Level State",
    accessorKey: "levelStateType",
  },
  {
    header: "Special Category",
    accessorKey: "specialCategory",
  },
  {
    header: "Score",
    accessorKey: "score",
  },
  {
    header: "Last Review Date",
    accessorKey: "lastReviewDate",
  },
  {
    header: "Next Review Days",
    accessorKey: "nextReviewDays",
  },
  {
    header: "Next Review Minutes",
    accessorKey: "nextReviewMinutes",
  },
];

export const TrackedEntriesTestPage: React.FC = () => {
  const [data, setData] = useState<TrackedEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const tableRef = useRef<TableRef<TrackedEntry>>(null);
  const detailsRef = useRef<TrackedEntriesDetailsRef>(null);

  const { refetch, isError, error } = useQuery({
    queryKey: ["trackedEntries"],
    queryFn: async () => {
      let result = await fetch(`${api_url}/TrackedEntry/all`);
      if (!result.ok) throw new Error("Network response was not ok");
      return result.json();
    },
    enabled: false,
  });

  const rowIsSelected = (row: any) => {
    if (detailsRef.current) {
      detailsRef.current.setES(row.ent_seq);
    }
  };

  useEffect(() => {
    const asyncEffect = async () => {
      try {
        const freshData = await refetch();

        if (isError) throw new Error(error.message);

        console.log(freshData.data);
        setData(freshData.data.data);
      } catch (error) {
        console.error("Error fetching data: ", error);
      } finally {
        setLoading(false);
      }
    };

    asyncEffect();
  }, []);

  return (
    <div className="h-full container grid grid-cols-2 gap-4">
      <Card>
        <CardHeader>
          <div className="flex place-content-between">
            <CardTitle>
              <h3>Table</h3>
            </CardTitle>
            {/* <Button onClick={showDetails}>Show details</Button> */}
          </div>
        </CardHeader>
        <CardContent>
          <DataTable
            ref={tableRef}
            columns={columns}
            data={data}
            showDetails={rowIsSelected}
          />
        </CardContent>
      </Card>
      <Card>
        <CardHeader>
          <CardTitle>
            <h3>Details</h3>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <TrackedEntriesDetails ref={detailsRef} />
        </CardContent>
      </Card>
    </div>
  );
};

// TrackedEntriesDetails

interface TrackedEntriesDetailsRef {
  // fetchData: (es: string) => void;
  setES: (es: string) => void;
}

const TrackedEntriesDetails = forwardRef(
  (props, ref: React.Ref<TrackedEntriesDetailsRef>) => {
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
              <DetailsSection
                title="TrackedEntry details"
                queryKeyName="trackedEntry"
                ent_seq={ent_seq}
                queryFn={async (): Promise<TrackedEntry> => {
                  const response = await fetch(
                    `${api_url}/TrackedEntry?ent_seq=${ent_seq}`
                  );
                  if (!response.ok)
                    throw new Error("TrackedEntry fetch not OK.");
                  const data =
                    (await response.json()) as ApiResponse<TrackedEntry>;
                  const parsed = TrackedEntrySchema.safeParse(data.data);
                  if (!parsed.success) {
                    console.error(parsed.error);
                    throw new Error("Failed to parse TrackedEntry");
                  }
                  return parsed.data;
                }}
              />
              <DetailsSection
                title="Entry details"
                queryKeyName="entry"
                ent_seq={ent_seq}
                queryFn={async (): Promise<Entry> => {
                  const response = await fetch(
                    `${api_url}/Entry?ent_seq=${ent_seq}`
                  );
                  if (!response.ok) throw new Error("Entry fetch not OK.");
                  const data = (await response.json()) as ApiResponse<Entry>;
                  const parsed = EntrySchema.safeParse(data.data);
                  if (!parsed.success) {
                    console.error(parsed.error);
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
  }
);
