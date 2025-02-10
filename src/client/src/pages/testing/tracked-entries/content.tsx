import { useEffect, useRef, useState } from "react";
import { columns } from "./columns";
import { DataTable, TableRef } from "./data-table";
import { useQuery } from "@tanstack/react-query";
import { ApiResponse, api_url } from "@/Api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Details, DetailsRef } from "./details";
import { TrackedEntry } from "@/types/TrackedEntry";

const TrackedEntriesContent: React.FC = () => {
  const [data, setData] = useState<TrackedEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const tableRef = useRef<TableRef<TrackedEntry>>(null);
  const detailsRef = useRef<DetailsRef>(null);

  const { refetch, isError, error } = useQuery({
    queryKey: ["weatherforecast"],
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
          <Details ref={detailsRef} />
        </CardContent>
      </Card>
    </div>
  );
};

export default TrackedEntriesContent;
