import React from 'react';
import { NavItem, NavLink } from 'reactstrap';
import Scrollspy from 'react-scrollspy';
import { useContent } from 'components/Content';

export const ContentMenu = () => {
  const content = useContent();
  
  return (
    <>
      <Scrollspy className="navbar-nav mr-auto" items={content.map((item) => item.path)} currentClassName="active" offset={56}>
        {
          content.map((section) => {
            const { path, text } = section;

            return (
              <NavItem key={path}>
                <NavLink href={`#${path}`} to={`#${path}`} onClick={(e) => e.preventDefault()}>{text}</NavLink>
              </NavItem>
            );
          })
        }
      </Scrollspy>
    </>
  );
};
