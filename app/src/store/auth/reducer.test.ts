import * as actions from './actions'
import { authReducer } from './reducer'

it("reduces the sign in request", () => {
    const expectedState = {
        data: "code",
        errors: undefined,
        loading: true,
    }

    expect(authReducer(undefined, actions.signIn("code")))
        .toEqual(expectedState)
});

it("reduces the success in request", () => {
    const startingState = {
        data: "",
        errors: undefined,
        loading: true,
    }
    const expectedState = {
        data: "token",
        errors: undefined,
        loading: false,
    }

    expect(authReducer(startingState, actions.signInSuccess("token")))
        .toEqual(expectedState)
});
