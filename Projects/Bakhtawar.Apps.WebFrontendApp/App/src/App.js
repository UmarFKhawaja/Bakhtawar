import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Count } from './components/Count';
import AuthorizedRoute from './components/api-authorization/AuthorizedRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './App.css';

export const App = () => (
  <Layout>
    <Route exact path='/' component={Home}/>
    <Route path='/count' component={Count}/>
    <AuthorizedRoute path='/fetch-data' component={FetchData}/>
    <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes}/>
  </Layout>
);