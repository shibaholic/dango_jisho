import { Separator } from "@/components/ui/separator";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import React from "react";

type row = {
  desc: string;
  status: string;
};

const featureRows: row[] = [
  {
    desc: "E-J dictionary search",
    status: "✅ Done",
  },
  {
    desc: "JWT token authentication and refresh",
    status: "✅ Done",
  },
  {
    desc: "Tag words and create study sets",
    status: "🛠️ In progress",
  },
  {
    desc: "Review words",
    status: "🛠️ In progress",
  },
  {
    desc: "Personal stats",
    status: "🗓️ Planned",
  },
  {
    desc: "Search optimization",
    status: "🗓️ Planned",
  },
  {
    desc: "Quizzes",
    status: "🗓️ Planned",
  },
  {
    desc: "Japanese to English review",
    status: "🗓️ Planned",
  },
  {
    desc: "Email verification",
    status: "🗓️ Planned",
  },
  {
    desc: "Import from Anki",
    status: "🗓️ Planned",
  },
];

const FeatureTable = () => {
  return (
    <Table className="text-xl border-b-[1px]">
      <TableHeader>
        <TableRow>
          <TableHead>Description</TableHead>
          <TableHead>Status</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {featureRows.map((row: row, index) => (
          <TableRow key={index} className="font-medium text-2xl">
            <TableCell className="content-center items-center self-center flex">
              {row.desc}
            </TableCell>
            <TableCell>{row.status}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

export default FeatureTable;
