import { useEffect, useRef, useState } from "react";
import { TrackedEntry, columns } from "./columns";
import { DataTable, TableRef } from "./data-table";
import { useQuery } from "@tanstack/react-query";
import { ApiResponse, api_url } from "@/Api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

const TrackedEntriesContent: React.FC = () => {
  const [data, setData] = useState<TrackedEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const tableRef = useRef<TableRef<TrackedEntry>>(null);

  const [ent_seq, setEnt_seq] = useState<string | null>(null);

  const { refetch, isError } = useQuery({
    queryKey: ["weatherforecast"],
    queryFn: async () => {
      let result = await fetch(`${api_url}/TrackedEntry`);
      if (!result.ok) throw new Error("Network response was not ok");
      return result.json();
    },
    enabled: false,
  });

  async function getData(): Promise<ApiResponse<TrackedEntry[]>> {
    const freshData = await refetch();

    if (isError) throw new Error("Network connection error");

    return freshData.data;
  }

  const getDetails = () => {
    // first get ent_seq from tableRef
    if (tableRef.current) {
      const currentRow = tableRef.current.getSelectedRow();
      if (currentRow) {
        setEnt_seq(currentRow.ent_seq);
      }
    }
  };

  useEffect(() => {
    const asyncEffect = async () => {
      try {
        const result = await getData();
        console.log(result);
        setData(result.data);
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
          <CardTitle>Table</CardTitle>
        </CardHeader>
        <CardContent>
          <Button onClick={getDetails}>Show details</Button>
          <DataTable ref={tableRef} columns={columns} data={data} />
        </CardContent>
      </Card>
      <Card>
        <CardHeader>
          <CardTitle>Details</CardTitle>
        </CardHeader>
        <CardContent>
          <p>
            ent_seq: <span>{ent_seq ? ent_seq : "null"}</span>
          </p>
        </CardContent>
      </Card>
    </div>
  );
};

export default TrackedEntriesContent;
