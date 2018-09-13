import { ConnectedRouter  } from 'connected-react-router';
import { createBrowserHistory } from 'history';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';

import 'rivet-uits/css/rivet.min.css';
import App from './App';
import configureStore from './configureStore'
 
const initialState = { auth : {
  code: undefined,
  error: undefined,
  loading: false,
  user: undefined
} }
const history = createBrowserHistory()
const store = configureStore(initialState, history)

ReactDOM.render(
  <Provider store={store}>
    <ConnectedRouter history={history}>
      <App />
    </ConnectedRouter>
  </Provider>,
  document.getElementById('root'));
