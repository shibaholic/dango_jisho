import { Separator } from "@/components/ui/separator";
import NavBar from "../../components/header/NavBar";
import { Helmet } from "react-helmet-async";
import { Table } from "@/components/ui/table";
import FeatureTable from "./FeatureTable";

function Landing() {
  return (
    <>
      <Helmet>
        <title>Dango Jisho</title>
      </Helmet>
      <div className="w-full flex flex-col items-center">
        <div className="flex flex-col gap-6 xl:w-[1000px] lg:w-[940px] md:w-[736px] w-[calc(100%-2rem)]">
          <NavBar />
          <Separator />
          <div className="flex flex-col gap-4">
            <h2>What is this?</h2>
            <p>
              This project is an attempt to create a spaced-repetition flashcard
              app designed for memorizing vocabulary. Currently,
              English-to-Japanese is implemented with English-to-other languages
              possibly also becoming available. It is inspired by jpdb.io.
            </p>
            <p>
              This project is currently in development. There are many planned
              features which can be seen in the table below.
            </p>
            <FeatureTable />
            <p>
              I am trying to improve on full-stack skills, so I am exploring
              ways to do things better. On the frontend side I have been using
              react-router, react-query, axios, react-hook-form, shadcn and
              TailwindCSS. On the backend side, I have used C# ASP.NET, Clean
              Architecture, Mediatr, EF Core, npgsql, XUnit, Moq and
              FluentAssertions.
            </p>
            <p>
              This project uses the{" "}
              <a href="http://www.edrdg.org/wiki/index.php/JMdict-EDICT_Dictionary_Project">
                JMdict
              </a>{" "}
              dictionary file.
            </p>
          </div>
          <Separator />
        </div>
      </div>
    </>
  );
}

export default Landing;
