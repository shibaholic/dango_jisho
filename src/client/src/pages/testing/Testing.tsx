import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import WeatherTestPage from "./pages/WeatherTestPage";
import { ReactNode, useEffect, useState } from "react";
import { TrackedEntriesTestPage } from "./pages/TrackedEntriesTestPage";
import RHKTestPage from "./pages/RHKTestPage";
import AuthTestPage from "./pages/AuthTestPage";
import { Helmet } from "react-helmet-async";
import TextTestPage from "./pages/Text";

function Testing() {
  return (
    <div className="m-4 h-full">
      <h1>Testing page</h1>
      {/* <Button>Click me</Button> */}
      <Tabs defaultValue="Auth" className="mt-4">
        <TabsList>
          <TabsTrigger value="Weather Forecast">Weather Forecast</TabsTrigger>
          <TabsTrigger value="Tracked Entries">Tracked Entries</TabsTrigger>
          <TabsTrigger value="RHK">React Hook Form Testing</TabsTrigger>
          <TabsTrigger value="Auth">Auth</TabsTrigger>
          <TabsTrigger value="Text">TextTest</TabsTrigger>
        </TabsList>

        <TabPageWrapper title="Weather Forecast">
          <WeatherTestPage />
        </TabPageWrapper>

        <TabPageWrapper title="Tracked Entries">
          <TrackedEntriesTestPage />
        </TabPageWrapper>

        <TabPageWrapper title="RHK">
          <RHKTestPage />
        </TabPageWrapper>

        <TabPageWrapper title="Auth">
          <AuthTestPage />
        </TabPageWrapper>

        <TabPageWrapper title="Text">
          <TextTestPage />
        </TabPageWrapper>
      </Tabs>
    </div>
  );
}

export default Testing;

interface TabPageProps {
  title: string;
  children?: ReactNode;
}

const TabPageWrapper: React.FC<TabPageProps> = ({ title, children }) => {
  return (
    <>
      <TabsContent value={title} className="p-4 h-full">
        <Helmet>
          <title>{title} - Testing</title>
        </Helmet>
        <h2 className="mb-4">{title}</h2>
        {children}
      </TabsContent>
    </>
  );
};
