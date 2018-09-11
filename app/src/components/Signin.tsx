
import * as queryString from 'query-string' 
import * as React from 'react';
import { connect } from 'react-redux'
import { Dispatch } from 'redux'

import { IApplicationState  } from '../store'
import { signIn  } from '../store/auth/actions'
import { IAuthState  } from '../store/auth/types'

interface ILocationProps {
    search: string;
}
interface ISigninProps {
    location: ILocationProps
}
// We can use `typeof` here to map our dispatch types to the props, like so.
interface IPropsFromDispatch {
    signIn: typeof signIn
}

class Signin extends React.Component<IAuthState & ISigninProps & IPropsFromDispatch> {

    public componentDidMount() {
        const queryParam = queryString.parse(this.props.location.search)
        console.log("Got code: ", queryParam.code)
        this.props.signIn(queryParam.code)
    }

    public render () {
        return (
            <>
                { this.props.loading && 
                  <p>Signing in...</p> }
                { !this.props.loading && 
                  <div>
                    <p>Data: {this.props.data}</p>
                    <p>Errors: {this.props.errors}</p> 
                  </div>}
            </>
        )
    };
}

// It's usually good practice to only include one context at a time in a connected component.
// Although if necessary, you can always include multiple contexts. Just make sure to
// separate them from each other to prevent prop conflicts.
const mapStateToProps = ({ auth }: IApplicationState) => ({
    data: auth.data,
    errors: auth.errors,
    loading: auth.loading
  })
  
  // mapDispatchToProps is especially useful for constraining our actions to the connected component.
  // You can access these via `this.props`.
  const mapDispatchToProps = (dispatch: Dispatch) => ({
    signIn: (code:string) => dispatch(signIn(code))
  })
  
// Now let's connect our component!
// With redux v4's improved typings, we can finally omit generics here.
export default connect(
    mapStateToProps,
    mapDispatchToProps
  )(Signin)
