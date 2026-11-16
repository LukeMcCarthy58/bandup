import { AppHeader } from "@/components/app-shell/app-header";
import { BottomNav } from "@/components/app-shell/bottom-nav";

/** Shell for the signed-in app: header, content column, and the mobile tab bar. */
export default function AppLayout({ children }: LayoutProps<"/">) {
  return (
    <>
      <AppHeader />
      <main
        id="main"
        tabIndex={-1}
        className="mx-auto w-full max-w-[600px] flex-1 px-4 pt-4 pb-[calc(5rem+env(safe-area-inset-bottom))] outline-none md:pb-10"
      >
        {children}
      </main>
      <BottomNav />
    </>
  );
}
