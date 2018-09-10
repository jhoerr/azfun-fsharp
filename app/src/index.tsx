import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { BrowserRouter as Router } from 'react-router-dom';
import { applyMiddleware, createStore } from 'redux';
import loggingMiddleware from 'redux-logger';

import 'rivet-uits/css/rivet.min.css';
import App from './App';
import reducer from './store';
 
const middleware = applyMiddleware(
  loggingMiddleware
  // thunkMiddleware
);
const defaultState = {};
const store = createStore(reducer, defaultState, middleware);

ReactDOM.render(
  <Provider store={store}>
    <Router>
      <App />
    </Router>
  </Provider>,
  document.getElementById('root'));
