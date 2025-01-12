import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import WeatherForecastContent from "./WeatherForecast";
import { ReactNode } from "react";
import TrackedEntriesContent from "./TrackedEntries";

function Testing() {
  return (
    <div className="m-4">
      <h1>Testing page</h1>
      {/* <Button>Click me</Button> */}
      <Tabs defaultValue="Weather Forecast" className="mt-4">
        <TabsList>
          <TabsTrigger value="Weather Forecast">Weather Forecast</TabsTrigger>
          <TabsTrigger value="Tracked Entries">Tracked Entries</TabsTrigger>
        </TabsList>

        <TabPageWrapper title="Weather Forecast">
          <WeatherForecastContent />
        </TabPageWrapper>

        <TabPageWrapper title="Tracked Entries">
          <TrackedEntriesContent />
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
    <TabsContent value={title} className="p-4">
      <h2 className="mb-4">{title}</h2>
      {children}
    </TabsContent>
  );
};
