import { Reducer } from 'redux'
import { FetchErrorReducer, FetchRequestReducer, FetchSuccessReducer } from '../common'
import { AuthActionTypes, IAuthState } from './types'

// Type-safe initialState!
const initialState: IAuthState = {
    data: undefined,
    error: undefined,
    loading: false,
    request: undefined
}

// Thanks to Redux 4's much simpler typings, we can take away a lot of typings on the reducer side,
// everything will remain type-safe.
const reducer: Reducer<IAuthState> = (state = initialState, action) => {
  switch (action.type) {
    case AuthActionTypes.SIGN_IN_REQUEST:
      return { ...state, 
        data: undefined,
        error: undefined,
        loading: false,
        request: undefined,
      }  
    case AuthActionTypes.POST_SIGN_IN_REQUEST: return FetchRequestReducer(state, action)
    case AuthActionTypes.POST_SIGN_IN_SUCCESS: return FetchSuccessReducer(state, action)
    case AuthActionTypes.POST_SIGN_IN_ERROR: return FetchErrorReducer(state, action)
    case AuthActionTypes.SIGN_OUT:
      return { ...state, 
          data: undefined,
          error: undefined,
          loading: false,
          request: undefined,
      }
    default: return state
  }
}

// Instead of using default export, we use named exports. That way we can group these exports
// inside the `index.js` folder.
export { 
  reducer as authReducer,
  initialState as initialAuthState
}
