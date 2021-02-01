import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Route, Redirect } from 'react-router-dom';
import { ApplicationPaths, QueryParameterNames } from 'constants/Authorization';
import { authorizationManager } from 'services/authorization-manager';

export const AuthorizedRoute = (props) => {
  const [isReady, setIsReady] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const populateAuthenticationState = useCallback(async () => {
    const isAuthenticated = await authorizationManager.isAuthenticated();

    setIsReady(true);
    setIsAuthenticated(isAuthenticated);
  }, []);

  const isAuthenticationChanged = useCallback(async () => {
    setIsReady(false);
    setIsAuthenticated(false);

    await populateAuthenticationState();
  }, [populateAuthenticationState]);

  useEffect(() => {
    const subscription = authorizationManager.subscribe(isAuthenticationChanged);

    populateAuthenticationState().then();

    return () => {
      authorizationManager.unsubscribe(subscription);
    };
  }, [populateAuthenticationState, isAuthenticationChanged]);

  const link = useRef(document.createElement('a'));

  link.current.href = props.path;

  const returnUrl = `${link.current.protocol}//${link.current.host}${link.current.pathname}${link.current.search}${link.current.hash}`;
  const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`;

  if (!isReady) {
    return <div/>;
  } else {
    const { component: Component, ...rest } = props;

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
};
