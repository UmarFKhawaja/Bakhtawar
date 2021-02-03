import React from 'react';
import { Footer } from './components/Footer';
import { Header } from './components/Header';
import { Section } from './components/Section';

import react from 'assets/react.png';

export const Home = () => (
  <>
    <Header/>
    <Section light>
      <h3 className="text-muted">Powered by</h3>
      <h1>
        <a href="https://reactjs.org" className="text-decoration-none" target="_blank" rel="noreferrer">
          <img src={react} alt="React"/>
          <span className="text-dark">React</span>
        </a>
      </h1>
      <h5>
        v17.0.1
      </h5>
    </Section>
    <Section id="#features">
      <h2>Feature list</h2>
      <p className="lead">
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Aut optio velit inventore,
        expedita quo laboriosam possimus ea consequatur vitae, doloribus consequuntur ex. Nemo assumenda
        laborum vel, labore ut velit dignissimos.
      </p>
    </Section>
    <Section id="#technology" light>
      <h2>Technology stack</h2>
      <p className="lead">
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Vero odio fugiat voluptatem
        dolor, provident officiis, id iusto! Obcaecati incidunt, qui nihil beatae magnam et repudiandae ipsa
        exercitationem, in, quo totam:
      </p>
      <ul>
        <li>Phasellus pharetra vel urna elementum sagittis. Proin faucibus nulla sed dapibus condimentum.</li>
        <li>Curabitur maximus, sapien rhoncus ultricies rutrum, sapien ante faucibus dui, ut venenatis augue quam id
          arcu. Nam tristique ullamcorper enim, a ullamcorper lectus eleifend eget. Morbi vel urna in turpis eleifend
          posuere et ac orci.
        </li>
        <li>Integer sed ultricies ipsum. Nulla facilisi. Vestibulum eu imperdiet augue. Phasellus at eleifend tellus.
        </li>
        <li>Pellentesque pellentesque magna eget quam finibus, ac malesuada ante ultrices. Proin at consectetur magna,
          ut sagittis ante.
        </li>
      </ul>
    </Section>
    <Section id="#contact">
      <h2>Contact us</h2>
      <p className="lead">
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Vero odio fugiat voluptatem
        dolor, provident officiis, id iusto! Obcaecati incidunt, qui nihil beatae magnam et repudiandae ipsa
        exercitationem, in, quo totam.
      </p>
    </Section>
    <Section id="#about">
      <h2>About us</h2>
      <p className="lead">
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Vero odio fugiat voluptatem
        dolor, provident officiis, id iusto! Obcaecati incidunt, qui nihil beatae magnam et repudiandae ipsa
        exercitationem, in, quo totam.
      </p>
    </Section>
    <Footer/>
  </>
);
