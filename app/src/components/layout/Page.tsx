import * as React from 'react';
import { connect } from 'react-redux'
import { Dispatch } from 'redux'
import { Footer, Header, HeaderIdentity, HeaderMenu, HeaderNavigation } from 'rivet-react';
import { IApplicationState  } from '../../store'
import { signOutRequest  } from '../../store/auth/actions'

export interface IPageProps {
    children?: React.ReactNode
    username?: string
}

// We can use `typeof` here to map our dispatch types to the props, like so.
interface IPropsFromDispatch {
  signOut: typeof signOutRequest
}

const Page: React.SFC<IPageProps & IPropsFromDispatch> = ({ username, signOut, children }) => (
  <>
    <Header title="IT Pro Database">
      { username &&
          <HeaderNavigation>
            <a href="#">Nav one</a>
            <HeaderMenu label="Nav two">
                <a href="#">Item one</a>
                <a href="#">Item two</a>
                <a href="#">Item three</a>
                <a href="#">Item four</a>
            </HeaderMenu>
          </HeaderNavigation>
      }
      { username &&
          <HeaderIdentity username={username} onLogout={signOut}/>
      }
      { !username &&
        <HeaderNavigation>
          <a href={`${process.env.REACT_APP_OAUTH2_AUTH_URL}?response_type=code&client_id=${process.env.REACT_APP_OAUTH2_CLIENT_ID}&redirect_uri=${process.env.REACT_APP_WEB_URL}/signin`}>Sign In</a>
        </HeaderNavigation>
      }
    </Header>
    <main id="main-content" className="rvt-m-top-xl rvt-m-left-xxl-md-up rvt-m-right-xxl-md-up rvt-m-bottom-xxl" style={{ flex: 1 }}>
      { children }
    </main>
    <Footer />
  </>
);

// It's usually good practice to only include one context at a time in a connected component.
// Although if necessary, you can always include multiple contexts. Just make sure to
// separate them from each other to prevent prop conflicts.
const mapStateToProps = ({ auth }: IApplicationState) => ({
  username: auth.user && auth.user.user_name,
})

// mapDispatchToProps is especially useful for constraining our actions to the connected component.
// You can access these via `this.props`.
const mapDispatchToProps = (dispatch: Dispatch) => ({
  signOut: () => dispatch(signOutRequest())
})

// Now let's connect our component!
// With redux v4's improved typings, we can finally omit generics here.
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Page)

