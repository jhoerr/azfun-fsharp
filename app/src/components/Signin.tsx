
import * as queryString from 'query-string' 
import * as React from 'react';
import { connect } from 'react-redux'
import { Dispatch } from 'redux'

import { IApplicationState  } from '../store'
import { postSignInRequest  } from '../store/auth/actions'
import { IAuthRequest, IAuthState,   } from '../store/auth/types'

interface ILocationProps {
    search: string;
}
interface ISigninProps {
    location: ILocationProps
}
// We can use `typeof` here to map our dispatch types to the props, like so.
interface IPropsFromDispatch {
    signInRequest: typeof postSignInRequest
}

class Signin extends React.Component<IAuthState & ISigninProps & IPropsFromDispatch> {

    public componentDidMount() {
        const queryParam = queryString.parse(this.props.location.search)
        this.props.signInRequest({ code: queryParam.code })
    }

    public render () {
        return (
            <>
                { this.props.loading &&
                  <p>Signing in...</p> }
                { this.props.data &&
                  <div>
                    <p>Username: {this.props.data.user_name} </p>
                    <p>Role: {this.props.data.user_role} </p>
                  </div> }
                { !this.props.loading && this.props.error &&
                  <div>
                    <p>Errors: {this.props.error}</p> 
                  </div> }
            </>
        )
    };
}

// It's usually good practice to only include one context at a time in a connected component.
// Although if necessary, you can always include multiple contexts. Just make sure to
// separate them from each other to prevent prop conflicts.
const mapStateToProps = ({ auth }: IApplicationState) : IAuthState => auth
  
  // mapDispatchToProps is especially useful for constraining our actions to the connected component.
  // You can access these via `this.props`.
  const mapDispatchToProps = (dispatch: Dispatch) => ({
    signInRequest: (request:IAuthRequest) => dispatch(postSignInRequest(request))
  })
  
// Now let's connect our component!
// With redux v4's improved typings, we can finally omit generics here.
export default connect<IAuthState, IPropsFromDispatch>(
    mapStateToProps,
    mapDispatchToProps
  )(Signin)
