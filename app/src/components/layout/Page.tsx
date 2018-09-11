import * as React from 'react';
import { Footer, Header, HeaderNavigation } from 'rivet-react';

export interface IPageProps {
    children?: React.ReactNode
}

const Page: React.SFC<IPageProps> = ({ children }) => (
  <>
    <Header title="IT Pro Database">
      <HeaderNavigation>
        <a href={`${process.env.REACT_APP_OAUTH2_AUTH_URL}?response_type=code&client_id=${process.env.REACT_APP_OAUTH2_CLIENT_ID}&redirect_uri=${process.env.REACT_APP_WEB_URL}/signin`}>Sign In</a>
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
