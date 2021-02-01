import React, { Component } from 'react';
import { Route, Redirect } from 'react-router-dom';
import { ApplicationPaths, QueryParameterNames } from 'constants/Authorization';
import { authorizationManager } from 'services/authorization-manager';

export class AuthorizedRoute extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      isReady: false,
      isAuthenticated: false
    };
  }
  
  async componentDidMount() {
    this.subscription = authorizationManager.subscribe(() => this.isAuthenticationChanged());

    await this.populateAuthenticationState();
  }
  
  componentWillUnmount() {
    authorizationManager.unsubscribe(this.subscription);
  }
  
  render() {
    const {
      path
    } = this.props;

    const {
      isReady,
      isAuthenticated
    } = this.state;

    const link = document.createElement('a');

    link.href = path;

    const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
    const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`;

    if (!isReady) {
      return <div/>;
    } else {
      const { component: Component, ...rest } = this.props;

      return (
        <Route {...rest}
               render={(props) => {
                 if (isAuthenticated) {
                   return <Component {...props} />;
                 } else {
                   return <Redirect to={redirectUrl}/>;
                 }
               }}
        />
      );
    }
  }

  async populateAuthenticationState() {
    const isAuthenticated = await authorizationManager.isAuthenticated();

    this.setState({ isReady: true, isAuthenticated });
  }

  async isAuthenticationChanged() {
    this.setState({ isReady: false, isAuthenticated: false });

    await this.populateAuthenticationState();
  }
}
