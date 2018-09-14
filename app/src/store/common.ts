import { AnyAction } from "redux";

// TYPES

// Declare state types with `readonly` modifier to get compile time immutability.
// https://github.com/piotrwitek/react-redux-typescript-guide#state-with-type-level-immutability
export interface IApiState<T> {
    readonly data?: T
    readonly error?: string
    readonly loading: boolean
}

// REDUCERS

export const FetchReducer = <T>(state:IApiState<T>, action:AnyAction) : IApiState<T> => (
    { ...state, 
        data: undefined,
        error: undefined,
        loading: true,
    }
)

export const FetchSuccessReducer = <T>(state:IApiState<T>, action:AnyAction) : IApiState<T> => (
    { ...state, 
        data: action.payload,
        error: undefined,
        loading: false,
    }
)

export const FetchErrorReducer = <T>(state:IApiState<T>, action:AnyAction) : IApiState<T> => (
    { ...state, 
        data: undefined,
        error: action.payload,
        loading: false,
    }
)
