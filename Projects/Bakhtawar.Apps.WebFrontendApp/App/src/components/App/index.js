import React from 'react';
import { BrowserRouter, Route } from 'react-router-dom';
import { Layout } from 'components/Layout';
import { Home } from 'components/Home';
import { AuthorizationRoutes } from 'components/AuthorizationRoutes';
import { ApplicationPaths } from 'constants/Authorization';

import './index.css';

export const App = () => (
  <BrowserRouter>
    <Layout>
      <Route exact path='/' component={Home}/>
      <Route path={ApplicationPaths.AuthorizationPrefix} component={AuthorizationRoutes}/>
    </Layout>
  </BrowserRouter>
);