import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { List } from 'rivet-react';
import { IApplicationState } from '../store';
import { profileFetchRequest } from '../store/profile/actions';
import { IProfileRequest, IProfileState } from '../store/profile/types';
import PageTitle from './layout/PageTitle';

interface IProfileProps {
    match: any
}
// We can use `typeof` here to map our dispatch types to the props, like so.
interface IPropsFromDispatch {
    profileFetchRequest: typeof profileFetchRequest
}

class Profile extends React.Component<IProfileState & IProfileProps & IPropsFromDispatch>{

    public componentDidMount() {
        this.props.profileFetchRequest({ username: this.props.match.params.username })
    }

    public render() {
        return (
            <>
                <PageTitle>Profile</PageTitle>
                { this.props.loading &&
                  <p>Fetching profile...</p> 
                }
                { this.props.data && 
                    <List>
                        <li><strong>Username:</strong> {this.props.data.username}</li>
                        <li><strong>Display Name:</strong> {this.props.data.username}</li>
                        <li><strong>Department:</strong> {this.props.data.department}</li>
                        <li><strong>Expertise:</strong> {this.props.data.expertise}</li>
                    </List>
                }
                { this.props.error &&
                  <div>
                    <p>Errors: {this.props.error}</p> 
                  </div> 
                }
            </>
        )
    }
}

// Although if necessary, you can always include multiple contexts. Just make sure to
// separate them from each other to prevent prop conflicts.
const mapStateToProps = ({ profile }: IApplicationState) => ({
    ...profile
  })
  
  // mapDispatchToProps is especially useful for constraining our actions to the connected component.
  // You can access these via `this.props`.
  const mapDispatchToProps = (dispatch: Dispatch) : IPropsFromDispatch => ({
    profileFetchRequest: (request: IProfileRequest) => dispatch(profileFetchRequest(request))
  })
  
// Now let's connect our component!
// With redux v4's improved typings, we can finally omit generics here.
export default connect(
    mapStateToProps,
    mapDispatchToProps
  )(Profile)
