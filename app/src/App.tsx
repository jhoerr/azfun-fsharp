import * as React from 'react';
import { Route, Switch } from 'react-router-dom';
import Home from './components/Home';
import Page from './components/layout/Page';
import Profile from './components/Profile';
import Signin from './components/Signin';

const App : React.SFC = () => (
  <Page>
    <Switch>
      <Route path="/" exact={true} component={Home} />
      <Route path="/signin" component={Signin} />
      <Route path="/profile/:username" component={Profile} />
    </Switch>
  </Page>
)

export default App;
