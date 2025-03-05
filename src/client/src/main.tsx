import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";

import { BrowserRouter, Routes, Route } from "react-router-dom";
import Testing from "./pages/testing/Testing.tsx";
import Landing from "./pages/landing/Landing.tsx";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import SearchResults from "./pages/search/SearchResults.tsx";
import Vocab from "./pages/vocab/Vocab.tsx";
import { AuthProvider } from "./utils/auth.tsx";
import { HelmetProvider } from "react-helmet-async";
import Tags from "./pages/tags/Tags.tsx";

const queryClient = new QueryClient();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <HelmetProvider>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Landing />} />
              <Route path="/search" element={<SearchResults />} />
              <Route path="/vocab/:ent_seq" element={<Vocab />} />
              <Route path="/tags" element={<Tags />} />
              <Route path="/testing" element={<Testing />} />
            </Routes>
          </BrowserRouter>
        </HelmetProvider>
      </AuthProvider>
    </QueryClientProvider>
  </StrictMode>
);
