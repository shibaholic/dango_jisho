import { Separator } from "@/components/ui/separator";
import NavBar from "../../components/header/NavBar";
import { Helmet } from "react-helmet-async";

function Landing() {
  return (
    <>
      <Helmet>
        <title>Chuui</title>
      </Helmet>
      <div className="w-full flex flex-col items-center">
        <div className="flex flex-col gap-6 xl:w-[1000px] lg:w-[940px] md:w-[736px] w-[calc(100%-2rem)]">
          <NavBar />
          <Separator />
          <div className="flex flex-col gap-4">
            <h2>Site traffic</h2>
            <p>
              Lorem ipsum dolor sit amet consectetur adipisicing elit.
              Consequatur sunt dignissimos eum nihil, ducimus assumenda,
              incidunt itaque natus, minus laudantium aut ea tenetur! Ipsam
              optio, cupiditate iure provident hic omnis.
            </p>
          </div>
          <Separator />
          <div className="flex flex-col gap-4">
            <h2>Study sets and tags</h2>
            <p>
              Lorem ipsum dolor sit amet consectetur adipisicing elit.
              Consequatur sunt dignissimos eum nihil, ducimus assumenda,
              incidunt itaque natus, minus laudantium aut ea tenetur! Ipsam
              optio, cupiditate iure provident hic omnis.
            </p>
          </div>
        </div>
      </div>
    </>
  );
}

export default Landing;
