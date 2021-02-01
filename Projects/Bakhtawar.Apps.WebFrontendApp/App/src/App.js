import React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Count } from './components/Count';
import { AuthorizedRoute } from './components/AuthorizedRoute';
import { AuthorizationRoutes } from './components/AuthorizationRoutes';
import { ApplicationPaths } from './constants/Authorization';

import './App.css';

export const App = () => (
  <Layout>
    <Route exact path='/' component={Home}/>
    <Route path='/count' component={Count}/>
    <AuthorizedRoute path='/fetch-data' component={FetchData}/>
    <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={AuthorizationRoutes}/>
  </Layout>
);