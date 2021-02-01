import React, { useCallback, useEffect, useState } from 'react';
import { QueryParameterNames, LogoutActions, ApplicationPaths } from 'constants/Authorization';
import { AuthenticationResultStatus, authorizationManager } from 'services/authorization-manager';

// The main responsibility of this component is to handle the user's logout process.
// This is the starting point for the logout process, which is usually initiated when a
// user clicks on the logout button on the LoginMenu component.
export const Logout = ({ action }) => {
  const [isReady, setIsReady] = useState(false);
  const [message, setMessage] = useState();

  const getReturnUrl = useCallback((state) => {
    const params = new URLSearchParams(window.location.search);
    const fromQuery = params.get(QueryParameterNames.ReturnUrl);

    if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }

    return (state && state.returnUrl) || fromQuery || `${window.location.origin}`;
  }, []);

  const navigateToReturnUrl = useCallback((returnUrl) => window.location.replace(returnUrl), []);

  const logout = useCallback(async (returnUrl) => {
    const isAuthenticated = await authorizationManager.isAuthenticated();

    if (isAuthenticated) {
      const result = await authorizationManager.signOut({ returnUrl });

      switch (result.status) {
        case AuthenticationResultStatus.Redirect:
          break;

        case AuthenticationResultStatus.Success:
          await navigateToReturnUrl(returnUrl);
          break;

        case AuthenticationResultStatus.Fail:
          setMessage(result.message);
          break;

        default:
          throw new Error('Invalid authentication result status.');
      }
    } else {
      setMessage('You successfully logged out!');
    }
  }, [navigateToReturnUrl]);

  const processLogoutCallback = useCallback(async () => {
    const url = window.location.href;
    const result = await authorizationManager.completeSignOut(url);

    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as the only time completeAuthentication finishes
        // is when we are doing a redirect sign in flow.
        throw new Error('Should not redirect.');

      case AuthenticationResultStatus.Success:
        await navigateToReturnUrl(getReturnUrl(result.state));
        break;

      case AuthenticationResultStatus.Fail:
        setMessage(result.message);
        break;

      default:
        throw new Error(`Invalid authentication result status '${result.status}'.`);
    }
  }, [getReturnUrl, navigateToReturnUrl]);

  const populateAuthenticationState = useCallback(async () => {
    await authorizationManager.isAuthenticated();

    setIsReady(true);
  }, []);

  useEffect(() => {
    switch (action) {
      case LogoutActions.Logout:
        if (!!window.history.state.state.local) {
          logout(getReturnUrl()).then();
        } else {
          // This prevents regular links to <app>/authentication/logout from triggering a logout
          setIsReady(true);
          setMessage('The logout was not initiated from within the page.');
        }
        break;

      case LogoutActions.LogoutCallback:
        processLogoutCallback().then();
        break;

      case LogoutActions.LoggedOut:
        setIsReady(true);
        setMessage('You successfully logged out!');
        break;

      default:
        throw new Error(`Invalid action '${action}'`);
    }

    populateAuthenticationState().then();

    return () => {
    };
  }, [action, getReturnUrl, logout, populateAuthenticationState, processLogoutCallback]);

  if (!isReady) {
    return (
      <div/>
    );
  }

  if (!!message) {
    return (
      <div>{message}</div>
    );
  } else {
    switch (action) {
      case LogoutActions.Logout:
        return (
          <div>Processing logout</div>
        );

      case LogoutActions.LogoutCallback:
        return (
          <div>Processing logout callback</div>
        );

      case LogoutActions.LoggedOut:
        return (
          <div>{message}</div>
        );

      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }
};
