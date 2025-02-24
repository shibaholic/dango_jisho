import LogInModal, { AuthModalHandle } from "@/components/auth/logInModal";
import { Button } from "@/components/ui/button";
import { fetchLogout, fetchUserAuth } from "@/utils/api";
import { useAuth } from "@/utils/auth";
import { useRef } from "react";

export default function AuthTestPage() {
  const { user, setUser, refreshToken: refetch } = useAuth();

  const authModalRef = useRef<AuthModalHandle>(null);

  async function clickLogout() {
    fetchLogout();

    // set user client-side to null
    setUser(null);

    // then depending on client-side authorization routing navigate somewhere or refresh page
  }

  return (
    <>
      <div className="flex flex-col w-[200px]">
        <p>{user ? "Username: " + user.username : "not logged in"}</p>
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={() => authModalRef.current?.open()}
        >
          Log in or sign up
        </Button>
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={() => refetch()}
        >
          Refresh token
        </Button>
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={clickLogout}
        >
          Logout
        </Button>
        <Button
          variant={"outline"}
          className="text-xl py-6"
          onClick={async () => {
            console.log((await fetchUserAuth()).data);
          }}
        >
          User auth endpoint
        </Button>
      </div>
      <LogInModal ref={authModalRef} />
    </>
  );
}
