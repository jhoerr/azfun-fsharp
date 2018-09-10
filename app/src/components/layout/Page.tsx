import * as React from 'react';
import { Link } from 'react-router-dom';
import { Footer, Header, HeaderNavigation } from 'rivet-react';

export interface IPageProps {
    children?: React.ReactNode
}

const Page: React.SFC<IPageProps> = ({ children }) => (
  <>
    <Header title="IT Pro Database">
      <HeaderNavigation>
        <Link to="/signin">Sign In</Link>
      </HeaderNavigation>
    </Header>
    <main id="main-content" className="rvt-m-top-xl rvt-m-left-xxl-md-up rvt-m-right-xxl-md-up rvt-m-bottom-xxl" style={{ flex: 1 }}>
      { children }
    </main>
    <Footer />
  </>
);

Page.displayName = 'Page';

export default Page;
