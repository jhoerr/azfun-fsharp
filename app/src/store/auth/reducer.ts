import { Reducer } from 'redux'
import { AuthActionTypes, IAuthState } from './types'

// Type-safe initialState!
const initialState: IAuthState = {
  code: undefined,
  data: undefined,
  error: undefined,
  loading: false,
}

// Thanks to Redux 4's much simpler typings, we can take away a lot of typings on the reducer side,
// everything will remain type-safe.
const reducer: Reducer<IAuthState> = (state = initialState, action) => {
  switch (action.type) {
    case AuthActionTypes.SIGN_IN: {
      return { ...state, 
        code: action.payload, 
        data: undefined,
        loading: true 
      }
    }
    case AuthActionTypes.SIGN_IN_SUCCESS: {
      return { ...state, 
        code: undefined, 
        data: action.payload,
        loading: false
      }
    }
    case AuthActionTypes.SIGN_IN_ERROR: {
      return { ...state, 
        code: undefined,
        error: action.payload,
        loading: false
      }
    }
    case AuthActionTypes.SIGN_OUT: {
        return { ...state, 
          data: undefined 
        }
    }
    default: {
      return state
    }
  }
}

// Instead of using default export, we use named exports. That way we can group these exports
// inside the `index.js` folder.
export { 
  reducer as authReducer,
  initialState as initialAuthState
}
